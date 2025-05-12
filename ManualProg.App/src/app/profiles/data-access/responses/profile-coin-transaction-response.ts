export interface IProfileCoinTransactionResponse {
  id: string;
  senderProfile: IProfileCoinTransactionResponseProfileData;
  receiverProfile: IProfileCoinTransactionResponseProfileData;
  ammount: number;
  created: Date;
}

export interface IProfileCoinTransactionResponseProfileData {
  id: string;
  name: string;
  hasImage: boolean;
}
