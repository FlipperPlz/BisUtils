using System.Net.Sockets;
using System.Reflection;
using BisUtils.DZA2S.Enumerations;
using BisUtils.DZA2S.Extensions;
using BisUtils.DZA2S.Models;

namespace BisUtils.DZA2S; 

public static class DzQueryClient {
    
    private static byte[] ReadQueryChallenge(UdpReceiveResult receiveResult) {
        using var challengeReader = new BinaryReader(new MemoryStream(receiveResult.Buffer));
        if (challengeReader.ReadSteamHeader() == false
            || challengeReader.ReadByte() != (byte) SteamQueryCode.ChallengeResponse)
            throw new Exception("Failed to read challenge response.");
                
        return challengeReader.ReadBytes(4);
    }

    private static async Task<byte[]> BuildQueryRequest<T>(CancellationToken cancellationToken = default) where T : IDzQuery {
        await using var requestWriter = new BinaryWriter(new MemoryStream());
        requestWriter.Write(IDzQuery.QueryHeader);
        requestWriter.Write(((byte[]) typeof(T).GetProperty("Magic", BindingFlags.Static)!.GetValue(null)! ??
                             throw new InvalidOperationException()));

        await using var requestStream = new MemoryStream();
        await requestWriter.BaseStream.CopyToAsync(requestStream, cancellationToken);

        return requestStream.ToArray();
    }
    
    public static async Task<DZInfoQuery?> QueryInformation(string ip, short port, 
        short clientSocket = 11551, int timeoutMs = 250, CancellationToken cancellationToken = default) {

        var infoRequest = await BuildQueryRequest<DZInfoQuery>(cancellationToken);
        byte[]? infoResponse;
        DZInfoQuery? response;

        using (var udpClient = new UdpClient(clientSocket)) {
            udpClient.Client.ReceiveTimeout = timeoutMs;

            await udpClient.SendAsync(infoRequest, ip, port, cancellationToken);

            var challengeResponse = ReadQueryChallenge(await udpClient.ReceiveAsync(cancellationToken));

            var fullRequest = new byte[infoRequest.Length + challengeResponse.Length];
            infoRequest.CopyTo(fullRequest, 0);
            challengeResponse.CopyTo(fullRequest, infoRequest.Length);

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
        short clientSocket = 11551, int timeoutMs = 250, CancellationToken cancellationToken = default) {
        byte[]? playerResponse;
        DZPlayerQuery? response;
        
        var playerRequest = await BuildQueryRequest<DZPlayerQuery>(cancellationToken);

        using (var udpClient = new UdpClient(clientSocket)) {
            udpClient.Client.ReceiveTimeout = timeoutMs;

            await udpClient.SendAsync(playerRequest, ip, port, cancellationToken);
            
            
            var challengeResponse = ReadQueryChallenge(await udpClient.ReceiveAsync(cancellationToken));
            var fullRequest = new byte[playerRequest.Length + challengeResponse.Length];
            playerRequest.CopyTo(fullRequest, 0);
            challengeResponse.CopyTo(fullRequest, playerRequest.Length);

            await udpClient.SendAsync(fullRequest, ip, port, cancellationToken);
            
            playerResponse = (await udpClient.ReceiveAsync(cancellationToken)).Buffer;
        }

        if (playerResponse is null) throw new Exception("Failed to receive a response");

        using (var reader = new BinaryReader(new MemoryStream(playerResponse))) {
            if (!reader.ReadSteamHeader() || reader.ReadByte() != (byte) SteamQueryCode.PlayersResponse)
                throw new Exception("Failed to parse player response");
            response = reader.ReadDZPlayerQuery();
        }

        return response;
    }
}