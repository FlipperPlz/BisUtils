namespace BisUtils.Core.Compression.Externals;

/// <summary>
/// LZSS compression variant compatible with BI games 
/// </summary>
/// <remarks>
/// Based on LZSS.c by Haruhiko Okumura
/// https://dev.gameres.com/Program/Other/LZSS.C
/// and its adaptation from bis-file-formats
/// https://github.com/jetelain/bis-file-formats/blob/master/BIS.Core/Compression/LzssCompression.cs
/// </remarks>
internal sealed class BisCompatibleLZSS {
    private const int N = 4096;
    private const int F = 18;
    private const byte FILL = 0x20;
    private const int THRESHOLD = 2;

    // index for root of binary search trees
    private readonly int NIL;

    // ring buffer of size N, with extra F-1 bytes to facilitate string comparison.
    private readonly byte[] textBuf;

    // These constitute binary search trees.
    // left children right children &amp; parents 
    private readonly int[] lson, rson, dad;

    private int matchPosition, matchLength; // of longest match.  These are set by the InsertNode() procedure.

    public BisCompatibleLZSS() {
        NIL = N;
        textBuf = new byte[N + F - 1];
        lson = new int[N + 1];
        rson = new int[N + 257];
        dad = new int[N + 1];
    }

    // Initialize trees.
    private void InitTree() {
        int i;

        /* For i = 0 to N - 1, rson[i] and lson[i] will be the right and
           left children of node i.  These nodes need not be initialized.
           Also, dad[i] is the parent of node i.  These are initialized to
           NIL (= N), which stands for 'not used.'
           For i = 0 to 255, rson[N + i + 1] is the root of the tree
           for strings that begin with character i.  These are initialized
           to NIL.  Note there are 256 trees. */

        for (i = N + 1; i <= N + 256; i++) {
            rson[i] = NIL;
        }

        for (i = 0; i < N; i++) {
            dad[i] = NIL;
        }
    }

    /* Inserts string of length F, text_buf[r..r+F-1], into one of the
       trees (text_buf[r]'th tree) and returns the longest-match position
       and length via the global variables match_position and match_length.
       If match_length = F, then removes the old node in favor of the new
       one, because the old one will be deleted sooner.
       Note r plays double role, as tree node and position in buffer. 
    */
    private void InsertNode(int r)
    {
        int i;
        var cmp = 1;
        matchLength = 0;

        var p = N + 1 + textBuf[r];

        rson[r] = lson[r] = NIL;
        for (; ; ) {
            if (cmp >= 0) {
                if (rson[p] != NIL) {
                    p = rson[p];
                }
                else {
                    rson[p] = r;
                    dad[r] = p;
                    return;
                }
            }
            else
            {
                if (lson[p] != NIL) {
                    p = lson[p];
                }
                else {
                    lson[p] = r;
                    dad[r] = p;
                    return;
                }
            }
            for (i = 1; i < F; i++) {
                if ((cmp = textBuf[r + i] - textBuf[p + i]) != 0) break;
            }

            if (i > matchLength) {
                matchPosition = p;
                if ((matchLength = i) >= F) break;
            }
        }
        dad[r] = dad[p];
        lson[r] = lson[p];
        rson[r] = rson[p];
        dad[lson[p]] = r;
        dad[rson[p]] = r;
        if (rson[dad[p]] == p) {
            rson[dad[p]] = r;
        }
        else {
            lson[dad[p]] = r;
        }
        
        dad[p] = NIL;  // remove p
    }

    // Deletes node p from tree
    private void DeleteNode(int p) {
        int q;

        if (dad[p] == NIL) return;  /* not in tree */
        if (rson[p] == NIL) {
            q = lson[p];
        }
        else if (lson[p] == NIL) {
            q = rson[p];
        }
        else {
            q = lson[p];
            if (rson[q] != NIL) {
                do {
                    q = rson[q];
                } while (rson[q] != NIL);
                rson[dad[q]] = lson[q];
                dad[lson[q]] = dad[q];
                lson[q] = lson[p];
                dad[lson[p]] = q;
            }
            rson[q] = rson[p];
            dad[rson[p]] = q;
        }
        dad[q] = dad[p];
        if (rson[dad[p]] == p) {
            rson[dad[p]] = q;
        }
        else {
            lson[dad[p]] = q;
        }
        dad[p] = NIL;
    }

    public int Encode(byte[] input, BinaryWriter output)
    {
        int i, len, last_match_length;
        byte mask, c;
        var codesize = 0;

        var codeBufIdx = mask = 1;
        var s = 0;
        var r = N - F;
        var stopPos = input.Length;
        var inputIdx = 0;
        Span<byte> codeBuf = stackalloc byte[17];

        InitTree();

        /* code_buf[1..16] saves eight units of code, 
         * and code_buf[0] works as eight flags, 
         * "1" representing that the unit is an unencoded letter (1 byte), 
         * "0" a position-and-length pair (2 bytes).  
         * Thus, eight units require at most 16 bytes of code. 
         */
        codeBuf[0] = 0;
        
        // Clear the buffer with any character that will appear often.
        for (i = s; i < r; i++) {
            textBuf[i] = FILL;
        }

        for (len = 0; len < F && inputIdx < stopPos; len++) {
            textBuf[r + len] = input[inputIdx++];  // Read F bytes into the last F bytes of the buffer
        }

        if (len == 0) {
            return 0;  /* text of size zero */
        }
        for (i = 1; i <= F; i++)
        {
            /* 
             * Insert the F strings, each of which begins with one or more 'space' characters.
             * Note the order in which these strings are inserted.
             * This way,  degenerate trees will be less likely to occur. 
             */
            InsertNode(r - i);
        }
        InsertNode(r);  /* Finally, insert the whole string just read.  The
                                  global variables match_length and match_position are set. */
        do
        {
            if (matchLength > len)
            {
                matchLength = len;  /* match_length
                                                                may be spuriously long near the end of text. */
            }
            if (matchLength <= THRESHOLD)
            {
                matchLength = 1;  /* Not long enough match.  Send one byte. */
                codeBuf[0] |= mask;  /* 'send one byte' flag */
                codeBuf[codeBufIdx++] = textBuf[r];  /* Send uncoded. */
            }
            else
            {
                /* Send position and length pair. Note match_length > THRESHOLD. */
                // The difference between common compression implementations and the BI variant is the match position format.
                var encoded_position = r - matchPosition & N - 1;
                codeBuf[codeBufIdx++] = (byte)encoded_position;
                codeBuf[codeBufIdx++] = (byte)(encoded_position >> 4 & 0xf0 | matchLength - (THRESHOLD + 1));
            }
            if ((mask <<= 1) == 0) /* Shift mask left one bit. */
            {
                output.Write(codeBuf[..codeBufIdx]); /* Send at most 8 units of code together */
                codesize += codeBufIdx;
                codeBuf[0] = 0;
                codeBufIdx = mask = 1;
            }
            last_match_length = matchLength;
            for (i = 0; i < last_match_length && inputIdx < stopPos; i++)
            {
                DeleteNode(s); // Delete old strings and
                c = input[inputIdx++];
                textBuf[s] = c; // read new bytes
                if (s < F - 1)
                {
                    textBuf[s + N] = c; // If the position is near the end of buffer, extend the buffer to make  string comparison easier.
                }
                s = s + 1 & N - 1;
                r = r + 1 & N - 1;
                /* Since this is a ring buffer, increment the position modulo N. */
                InsertNode(r);    /* Register the string in text_buf[r..r+F-1] */
            }

            while (i++ < last_match_length)
            {
                /* After the end of text, */
                DeleteNode(s); /* no need to read, but */
                s = s + 1 & N - 1;
                r = r + 1 & N - 1;
                --len;
                if (len != 0) InsertNode(r);        /* buffer may not be empty. */
            }
        } while (len > 0); /* until length of string to be processed is zero */

        if (codeBufIdx > 1)
        {
            /* Send remaining code. */
            output.Write(codeBuf[..codeBufIdx]);
            codesize += codeBufIdx;
        }
        output.Flush();
        return codesize;
    }
}