namespace BisUtils.Core.Compression;


/// <summary>
///     Bis Lzss
/// </summary>
/// Author: https://github.com/rvost Thank You So Much
public sealed class BisCompatibleLzss
{
    public static BisCompatibleLzss Compressor { get; } = new BisCompatibleLzss();
    private const int N = 4096, F = 18, Threshold = 2;
    private const byte Fill = 0x20;

    /// <summary>
    ///     Index for root of binary search trees
    /// </summary>
    private readonly int nil = N;

    /// <summary>
    ///     These constitute binary search trees.
    /// </summary>
    private readonly int[] previousChildren = new int[N + 1], nextChildren = new int[N + 257], parents = new int[N + 1];

    /// <summary>
    ///     The ring buffer of size N with extra F-1 bytes to facilitate string comparison.
    /// </summary>
    private readonly byte[] textBuffer = new byte[N + F - 1];

    /// <summary>
    ///     The length and position of the longest match found.
    ///     These are set by the InsertNode() procedure.
    /// </summary>
    private int matchPosition, matchLength;

    public void Decode(BinaryReader reader, BinaryWriter output, uint length)
    {
        var r = N - F;
        var flags = 0;
        var stopPos = reader.BaseStream.Position + length;
        InitTree();
        for (var i = 0; i < r; i++)
        {
            textBuffer[i] = Fill;
        }

        while (reader.BaseStream.Position < stopPos)
        {
            if (((flags >>= 1) & 256) != 256)
            {
                flags = reader.ReadByte() | 0xff00;  // Set flags with the 8th bit.  1 left shift = 1 byte read.
            }

            if ((flags & 1) == 1)
            {
                var c = reader.ReadByte();
                output.Write(c);
                textBuffer[r++] = c;
                r &= N - 1;
            }
            else
            {
                int i = reader.ReadByte(), j = reader.ReadByte();

                i |= (j & 0xf0) << 4;
                j = (j & 0xf) + Threshold;

                for (var k = 0; k <= j; k++)
                {
                    var c = textBuffer[(i + k) & (N - 1)];
                    output.Write(c);
                    textBuffer[r++] = c;
                    r &= N - 1;
                }
            }
        }
    }

    public Stream Encode(Stream inputStream, out uint outputSize)
    {
        if (!inputStream.CanRead)
        {
            throw new IOException("Cannot read from the provided stream.");
        }

        using var memoryStream = new MemoryStream();
        inputStream.CopyTo(memoryStream);

        var encodedStream = new MemoryStream();
        using var binaryWriter = new BinaryWriter(encodedStream);

        outputSize = Encode(memoryStream.ToArray(), binaryWriter);

        encodedStream.Seek(0, SeekOrigin.Begin);
        return encodedStream;
    }

    public uint Encode(Stream inputStream, BinaryWriter output)
    {
        if (!inputStream.CanRead)
        {
            throw new IOException("Cannot read from the provided stream.");
        }

        using var memoryStream = new MemoryStream();
        inputStream.CopyTo(memoryStream);
        return Encode(memoryStream.ToArray(), output);
    }

    public uint Encode(byte[] input, BinaryWriter output)
    {
        int i, len;
        byte mask;
        uint codeSize = 0;

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
        for (i = s; i < r; i++)
        {
            textBuffer[i] = Fill;
        }

        for (len = 0; len < F && inputIdx < stopPos; len++)
        {
            textBuffer[r + len] = input[inputIdx++]; // Read F bytes into the last F bytes of the buffer
        }

        if (len == 0)
        {
            return 0; /* text of size zero */
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

        InsertNode(r); /* Finally, insert the whole string just read.  The
                                  global variables match_length and match_position are set. */
        do
        {
            if (matchLength > len)
            {
                matchLength = len; /* match_length
                                                                may be spuriously long near the end of text. */
            }

            if (matchLength <= Threshold)
            {
                matchLength = 1; /* Not long enough match.  Send one byte. */
                codeBuf[0] |= mask; /* 'send one byte' flag */
                codeBuf[codeBufIdx++] = textBuffer[r]; /* Send decoded. */
            }
            else
            {
                /* Send position and length pair. Note match_length > THRESHOLD. */
                var encoded_position = (r - matchPosition) & (N - 1);
                codeBuf[codeBufIdx++] = (byte)encoded_position;
                codeBuf[codeBufIdx++] = (byte)(((encoded_position >> 4) & 0xf0) | (matchLength - (Threshold + 1)));
            }

            if ((mask <<= 1) == 0) /* Shift mask left one bit. */
            {
                output.Write(codeBuf[..codeBufIdx]); /* Send at most 8 units of code together */
                codeSize += codeBufIdx;
                codeBuf[0] = 0;
                codeBufIdx = mask = 1;
            }

            var lastMatchLength = matchLength;
            for (i = 0; i < lastMatchLength && inputIdx < stopPos; i++)
            {
                DeleteNode(s); // Delete old strings and
                var c = input[inputIdx++];
                textBuffer[s] = c; // read new bytes
                if (s < F - 1)
                {
                    textBuffer[s + N] =
                        c; // If the position is near the end of buffer, extend the buffer to make  string comparison easier.
                }

                s = (s + 1) & (N - 1);
                r = (r + 1) & (N - 1);
                /* Since this is a ring buffer, increment the position modulo N. */
                InsertNode(r); /* Register the string in text_buf[r..r+F-1] */
            }

            while (i++ < lastMatchLength)
            {
                /* After the end of text, */
                DeleteNode(s); /* no need to read, but */
                s = (s + 1) & (N - 1);
                r = (r + 1) & (N - 1);
                --len;
                if (len != 0)
                {
                    InsertNode(r); /* buffer may not be empty. */
                }
            }
        } while (len > 0); /* until length of string to be processed is zero */

        if (codeBufIdx > 1)
        {
            /* Send remaining code. */
            output.Write(codeBuf[..codeBufIdx]);
            codeSize += codeBufIdx;
        }

        output.Flush();
        return codeSize;
    }

    /// <summary>
    ///     Initializes the binary search trees used by the compression algorithm.
    /// </summary>
    private void InitTree()
    {
        int i;
        /*
         For i = 0 to N - 1, NextChildren[i] and PreviousChildren[i] will be the right and
         left children of node i.  These nodes need not be initialized.
         Also, Parents[i] is the parent of node i.  These are initialized to
         Nil (= N), which stands for 'not used.'
         For i = 0 to 255, NextChildren[N + i + 1] is the root of the tree
         for strings that begin with character i.  These are initialized
         to Nil.  Note there are 256 trees.
       */

        for (i = N + 1; i <= N + 256; i++)
        {
            nextChildren[i] = nil;
        }

        for (i = 0; i < N; i++)
        {
            parents[i] = nil;
        }
    }

    /// <summary>
    ///     Inserts string of length F, TextBuffer[node..node+F-1], into one of the
    ///     trees (TextBuffer[node]'th tree) and returns the longest-match position
    ///     and length via the global variables MatchPosition and MatchLength.
    ///     If MatchLength = F, then removes the old node in favor of the new
    ///     one, because the old one will be deleted sooner.
    /// </summary>
    /// <param name="node">plays double role, as tree node and position in buffer.</param>
    private void InsertNode(int node)
    {
        var cmp = 1;
        matchLength = 0;

        var p = N + 1 + textBuffer[node];

        nextChildren[node] = previousChildren[node] = nil;
        for (;;)
        {
            if (cmp >= 0)
            {
                if (nextChildren[p] != nil)
                {
                    p = nextChildren[p];
                }
                else
                {
                    nextChildren[p] = node;
                    parents[node] = p;
                    return;
                }
            }
            else
            {
                if (previousChildren[p] != nil)
                {
                    p = previousChildren[p];
                }
                else
                {
                    previousChildren[p] = node;
                    parents[node] = p;
                    return;
                }
            }

            int i;
            for (i = 1; i < F; i++)
            {
                if ((cmp = textBuffer[node + i] - textBuffer[p + i]) != 0)
                {
                    break;
                }
            }

            if (i <= matchLength)
            {
                continue;
            }

            matchPosition = p;
            if ((matchLength = i) >= F)
            {
                break;
            }
        }

        parents[node] = parents[p];
        previousChildren[node] = previousChildren[p];
        nextChildren[node] = nextChildren[p];
        parents[previousChildren[p]] = node;
        parents[nextChildren[p]] = node;
        if (nextChildren[parents[p]] == p)
        {
            nextChildren[parents[p]] = node;
        }
        else
        {
            previousChildren[parents[p]] = node;
        }

        parents[p] = nil;
    }

    /// <summary>
    ///     Deletes node n from tree
    /// </summary>
    /// <param name="n">Node to remove from tree</param>
    private void DeleteNode(int n)
    {
        int q;

        if (parents[n] == nil)
        {
            return; /* not in tree */
        }

        if (nextChildren[n] == nil)
        {
            q = previousChildren[n];
        }
        else if (previousChildren[n] == nil)
        {
            q = nextChildren[n];
        }
        else
        {
            q = previousChildren[n];
            if (nextChildren[q] != nil)
            {
                do
                {
                    q = nextChildren[q];
                } while (nextChildren[q] != nil);

                nextChildren[parents[q]] = previousChildren[q];
                parents[previousChildren[q]] = parents[q];
                previousChildren[q] = previousChildren[n];
                parents[previousChildren[n]] = q;
            }

            nextChildren[q] = nextChildren[n];
            parents[nextChildren[n]] = q;
        }

        parents[q] = parents[n];
        if (nextChildren[parents[n]] == n)
        {
            nextChildren[parents[n]] = q;
        }
        else
        {
            previousChildren[parents[n]] = q;
        }

        parents[n] = nil;
    }
}
