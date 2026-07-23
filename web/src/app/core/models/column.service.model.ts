export interface ReorderColumnsRequest {
  projectId: number;
  orderedColumnIds: number[]; // 👈 Backend'in beklediği dizi formatı
}