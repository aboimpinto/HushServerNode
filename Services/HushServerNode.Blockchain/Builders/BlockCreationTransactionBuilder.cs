// using HushServerNode.Blockchain.Model;

// namespace HushServerNode.Blockchain.Builders
// {
//     public class BlockCreationTransactionBuilder
//     {
//         private string _blockId = string.Empty;
//         private string _destinationAddress = string.Empty;

//         public string BlockCreationTransationId { get; } = "{8e29c7c1-f2d8-4ff3-9d97-e927e3f40c79}";

//         public BlockCreationTransactionBuilder WithDestinationAddress(string destinationAddress)
//         {
//             this._destinationAddress = destinationAddress;
//             return this;
//         }

//         public BlockCreationTransaction Build()
//         {
//             var transaction = new BlockCreationTransaction
//             {
//                 DestinationAddress = this._destinationAddress,
//                 Reward = 0.5                                        // TODO: This should be configurable by the network.
//             };

//             return transaction;
//         }
//     }
// }