using System.Net.Sockets;
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
        requestWriter.Write((byte[]) typeof(T).GetMethod("GetSendMagic")!.Invoke(null, null)! ??
                            throw new InvalidOperationException());
        
        return requestStream.ToArray();
    }
    
    private static byte[] GetReceiveHeader<T>() where T : IDzQuery =>
        (byte[])typeof(T).GetMethod("GetReceiveMagic")!.Invoke(null, null)! ??
        throw new InvalidOperationException();

    private static async Task<byte[]> CompleteChallenge<T>(UdpClient udpClient, string ip, short port, CancellationToken cancellationToken = default) where T : IDzQuery {
        var challengeResultOrQuery =  (await udpClient.ReceiveAsync(cancellationToken)).Buffer;

        while (challengeResultOrQuery[4] != GetReceiveHeader<T>()[0]) {
            if (challengeResultOrQuery[4] != (byte)SteamQueryCode.ChallengeResponse)
                throw new Exception($"Expected a challenge response. Instead got {challengeResultOrQuery[4]:x8}");
            byte[] fullRequest;
            var request = await BuildQueryRequest<T>();
            switch (typeof(T).Name) {
                case nameof(DZInfoQuery):
                    fullRequest = new byte[request.Length + 4];
                    request.CopyTo(fullRequest, 0);
                    ReadQueryChallenge(challengeResultOrQuery).CopyTo(fullRequest, request.Length);
                    break;
                default:
                    fullRequest = new byte[request.Length];
                    request[..5].CopyTo(fullRequest, 0);
                    ReadQueryChallenge(challengeResultOrQuery).CopyTo(fullRequest, request.Length - 4);
                    break;
            }
            await udpClient.SendAsync(fullRequest, ip, port, cancellationToken);
            challengeResultOrQuery = (await udpClient.ReceiveAsync(cancellationToken)).Buffer;
        }

        return challengeResultOrQuery;
    }

    public static async Task<T> QueryServer<T>(string ip, short port, short clientSocket = 11551,
        int timeoutMs = 100000, CancellationToken cancellationToken = default) where T : IDzQuery {
        
        var request = await BuildQueryRequest<T>();
        byte[] response;
        T serializedResponse;

        using (var udpClient = new UdpClient(clientSocket)) {
            udpClient.Client.ReceiveTimeout = timeoutMs;

            await udpClient.SendAsync(request, ip, port, cancellationToken);

            response = await CompleteChallenge<T>(udpClient, ip, port, cancellationToken);
        }

        using (var reader = new BinaryReader(new MemoryStream(response))) {
            if(!reader.ReadSteamHeader() || reader.ReadByte() != GetReceiveHeader<T>()[0])
                throw new Exception("Failed to parse player response");
                
            serializedResponse = reader.ReadSteamResponse<T>();
        }

        return serializedResponse;
    } 
    
    public static async Task<DZInfoQuery?> QueryInformation(string ip, short port, short clientSocket = 11551, int timeoutMs = 100000, CancellationToken cancellationToken = default) =>
        await QueryServer<DZInfoQuery>(ip, port, clientSocket, timeoutMs, cancellationToken);
    
    public static async Task<DZRulesQuery?> QueryRules(string ip, short port, short clientSocket = 11551, int timeoutMs = 100000, CancellationToken cancellationToken = default) =>
        await QueryServer<DZRulesQuery>(ip, port, clientSocket, timeoutMs, cancellationToken);

    public static async Task<DZPlayerQuery?> QueryPlayers(string ip, short port, short clientSocket = 11551, int timeoutMs = 100000, CancellationToken cancellationToken = default) =>
        await QueryServer<DZPlayerQuery>(ip, port, clientSocket, timeoutMs, cancellationToken);
}
