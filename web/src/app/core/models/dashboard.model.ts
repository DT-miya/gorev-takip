export interface AssignedTask {
  id: number;
  title: string;
  priority: string;
  dueDate: string | null;
  columnName: string;
  projectId: number;
  projectName: string;
}