import { UserRole } from "../../../shared/enums/user-role";

export interface IUserResponse {
  id: string;
  username: string;
  role: UserRole;
  profile: IUserResponseProfileData | null;
  created: Date;
}

export interface IUserResponseProfileData {
  id: string;
  name: string;
  hasImage: boolean;
}
