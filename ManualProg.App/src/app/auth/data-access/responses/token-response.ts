import { UserRole } from "../../../shared/enums/user-role";

export interface ITokenResponse {
  accessToken: string;
  refreshToken: string;
  userId: string;
  username: string;
  userRole: UserRole;
  profileId?: string | null;
}
