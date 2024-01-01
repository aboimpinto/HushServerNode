using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Olimpo;

namespace HushServerNode.Blockchain.Model;

public class Block: IBlock
{
    public string BlockId { get; } = string.Empty;

    public string TimeStamp { get; } = string.Empty;

    public IEnumerable<TransactionBase> Transactions { get; set; }

    public double Index { get; }

    public string Hash { get; private set; } = string.Empty;

    public string PreviousBlockId { get; } = string.Empty;

    public string NextBlockId { get; } = string.Empty;

    public string Signature { get; private set; } = string.Empty;

    public Block(string blockId, string previousBlockId, string nextBlockId, double index)
    {
        this.Transactions = new List<TransactionBase>();

        this.TimeStamp = DateTime.UtcNow.ToString();

        this.PreviousBlockId = previousBlockId;
        this.NextBlockId = nextBlockId;

        this.BlockId = blockId;
        this.Index = index;
    }

    public void FinalizeBlock()
    {
        this.Hash = this.GetHashCode().ToString();
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

    public bool CheckBlockSignature(string signingAddress, TransactionBaseConverter transactionBaseConverter)
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
