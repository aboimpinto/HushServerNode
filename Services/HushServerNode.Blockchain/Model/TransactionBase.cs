using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Olimpo;

namespace HushServerNode.Blockchain.Model;

public abstract class TransactionBase : ITransaction
{
    public string TransactionId { get; } = string.Empty;

    public string Type { get; } = string.Empty;

    public string BlockId { get; } = string.Empty;

    public string Issuer { get; } = string.Empty;

    public string Signature { get; private set; } = string.Empty;

    public TransactionBase(string transactionType, string blockId, string transactionIssuer)
    {
        this.Type = transactionType;
        this.BlockId = blockId;
        this.Issuer = transactionIssuer;

        this.TransactionId = Guid.NewGuid().ToString();
    }

    public void Sign(string privateKey, TransactionBaseConverter transactionBaseConverter)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { ExcludeSignatureProperty }
            },
            Converters = { transactionBaseConverter }
        };

        var json = JsonSerializer.Serialize(this, jsonOptions);

        this.Signature = SigningKeys.SignMessage(json, privateKey);
    }

    public bool CheckTransactionSignature(string signingAddress, TransactionBaseConverter transactionBaseConverter)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { ExcludeSignatureProperty }
            },
            Converters = { transactionBaseConverter }
        };

        var json = JsonSerializer.Serialize(this, jsonOptions);
        return SigningKeys.VerifySignature(json, this.Signature, signingAddress);
    }

    static void ExcludeSignatureProperty(JsonTypeInfo jsonTypeInfo)
    {
        if (jsonTypeInfo.Kind != JsonTypeInfoKind.Object)
            return;

        foreach (JsonPropertyInfo property in jsonTypeInfo.Properties)
        {
            if (property.Name.Contains("Signature"))
            {
                property.Get = null;
                property.Set = null;
            }
        }
    }
}