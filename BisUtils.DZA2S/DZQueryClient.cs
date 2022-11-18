using System.Net.Sockets;
using System.Reflection;
using BisUtils.DZA2S.Enumerations;
using BisUtils.DZA2S.Extensions;
using BisUtils.DZA2S.Models;

namespace BisUtils.DZA2S; 

public static class DzQueryClient {
    
    private static byte[] ReadQueryChallenge(byte[] bytes) {
        using var challengeReader = new BinaryReader(new MemoryStream(bytes));
        if (challengeReader.ReadSteamHeader() == false
            || challengeReader.ReadByte() != (byte) SteamQueryCode.ChallengeResponse)
            throw new Exception("Failed to read challenge response.");
                
        return challengeReader.ReadBytes(4);
    }

    private static async Task<byte[]> BuildQueryRequest<T>() where T : IDzQuery {
        var requestStream = new MemoryStream();
        await using var requestWriter = new BinaryWriter(requestStream);
        requestWriter.Write(IDzQuery.QueryHeader);
        requestWriter.Write((byte[]) typeof(T).GetMethod("GetMagic")!.Invoke(null, null)! ??
                            throw new InvalidOperationException());
        
        return requestStream.ToArray();
    }
    
    public static async Task<DZInfoQuery?> QueryInformation(string ip, short port, 
        short clientSocket = 11551, int timeoutMs = 100000, CancellationToken cancellationToken = default) {

        var infoRequest = await BuildQueryRequest<DZInfoQuery>();
        byte[]? infoResponse;
        DZInfoQuery? response;

        using (var udpClient = new UdpClient(clientSocket)) {
            udpClient.Client.ReceiveTimeout = timeoutMs;

            await udpClient.SendAsync(infoRequest, ip, port, cancellationToken);

            var challengeResult = (await udpClient.ReceiveAsync(cancellationToken));

            var fullRequest = new byte[infoRequest.Length + 4];
            infoRequest.CopyTo(fullRequest, 0);
            ReadQueryChallenge(challengeResult.Buffer).CopyTo(fullRequest, infoRequest.Length);
            
            await udpClient.SendAsync(fullRequest, ip, port, cancellationToken);
            
            infoResponse = (await udpClient.ReceiveAsync(cancellationToken)).Buffer;
        }

        if (infoResponse is null) throw new Exception("Failed to receive a response");
        
        using (var reader = new BinaryReader(new MemoryStream(infoResponse))) {
            if (!reader.ReadSteamHeader() || reader.ReadByte() != (byte) SteamQueryCode.InfoResponse)
                throw new Exception("Failed to parse info response");
            response = reader.ReadDZInfoQuery();
        }

        return response;
    }

    public static async Task<DZPlayerQuery?> QueryPlayerInformation(string ip, short port, 
        short clientSocket = 11551, int timeoutMs = 100000, CancellationToken cancellationToken = default) {
        byte[]? playerResponse;
        DZPlayerQuery? response;
        
        var playerRequest = await BuildQueryRequest<DZPlayerQuery>();
        
        using (var udpClient = new UdpClient(clientSocket)) {
            udpClient.Client.ReceiveTimeout = timeoutMs;

            await udpClient.SendAsync(playerRequest, ip, port, cancellationToken);
            
            var challengeResult = (await udpClient.ReceiveAsync(cancellationToken));

            var fullRequest = new byte[playerRequest.Length];
            playerRequest[..5].CopyTo(fullRequest, 0);
            ReadQueryChallenge(challengeResult.Buffer).CopyTo(fullRequest, playerRequest.Length - 4);

            await udpClient.SendAsync(fullRequest, ip, port, cancellationToken);
            
            playerResponse = (await udpClient.ReceiveAsync(cancellationToken)).Buffer;
        }

        if (playerResponse is null) throw new Exception("Failed to receive a response");

        using (var reader = new BinaryReader(new MemoryStream(playerResponse))) {
            if (!reader.ReadSteamHeader() || reader.ReadByte() != (byte) SteamQueryCode.PlayersResponse)
                throw new Exception("Failed to parse player response");
            await Console.Out.WriteLineAsync(Convert.ToBase64String(reader.ReadBytes((int) (reader.BaseStream.Length - reader.BaseStream.Position))));
            response = reader.ReadDZPlayerQuery();
        }

        return response;
    }
}