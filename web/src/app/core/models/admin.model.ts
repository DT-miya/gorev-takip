export interface AdminUser {
    id: number;
    fullName: string;
    email: string;
    role: string;
}

export interface AdminStats {
    userCount: number;
    projectCount: number;
    taskCount: number;
}