export interface IRegisterRequest {
  username: string;
  password: string;
  profile: IRegisterRequestProfileData;
}

export interface IRegisterRequestProfileData {
  name: string;
  description?: string | null;
  image?: File | null;
}
