export interface Project {
  id: number;
  name: string;
  description?: string;  
  ownerName: string;
  memberCount: number;
}

export interface CreateProjectRequest {
  name: string;
  description?: string;
}

export interface Member {
  userId: number;
  fullName: string;
  email: string;
  role: string;
}