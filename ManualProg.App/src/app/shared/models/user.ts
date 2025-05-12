import { UserRole } from "../enums/user-role";

export interface IUser {
  id: string;
  username: string;
  role: UserRole;
  profileId?: string | null;
}
