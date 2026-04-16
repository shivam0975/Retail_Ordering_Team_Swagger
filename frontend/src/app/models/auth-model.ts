export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
}

export interface AdminRegisterRequest {
  name: string;
  email: string;
  password: string;
  adminSecret: string;
}

export interface UserSession {
  userId: number;
  name: string;
  email: string;
  role: string;
  token: string;
}
