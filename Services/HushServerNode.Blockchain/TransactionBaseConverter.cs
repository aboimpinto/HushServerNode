using System.Text.Json;
using System.Text.Json.Serialization;
using HushServerNode.Blockchain.Model;
using HushServerNode.Blockchain.TransactionConvertersStrategies;

namespace HushServerNode.Blockchain;

public class TransactionBaseConverter : JsonConverter<TransactionBase>
{
    private readonly IEnumerable<ISpecificTransactionDeserializer> _transactionDeserializer;
        
    public TransactionBaseConverter(IEnumerable<ISpecificTransactionDeserializer> transactionDeserializer)
    {
        this._transactionDeserializer = transactionDeserializer;
    }

    public override TransactionBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // TODO: all this logic will be replaces by the ISpecificTransactionDeserliazer

        using (var jsonDocument = JsonDocument.ParseValue(ref reader))
        {
            var element = jsonDocument.RootElement;
            var transactionType = element.GetProperty("Type").GetUInt32();

            if (transactionType == 1)
            {
                return JsonSerializer.Deserialize<BlockCreationTransaction>(element.GetRawText());
            }
        }

        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, TransactionBase value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
    
}
