import { UserRole } from "../../../shared/enums/user-role";

export interface ICreateUserRequest {
  username: string;
  password: string;
  role: UserRole;
}
