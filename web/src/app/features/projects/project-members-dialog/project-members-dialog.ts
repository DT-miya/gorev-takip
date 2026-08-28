import { Component, OnInit, signal, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { ProjectService } from '@services/project.service';
import { Member } from '@models/project.model';

@Component({
  selector: 'app-project-members-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatListModule,
    MatIconModule
  ],
  templateUrl: './project-members-dialog.html',
  styleUrl: './project-members-dialog.css'
})
export class ProjectMembersDialog implements OnInit {
  members = signal<Member[]>([]);
  newEmail = '';
  loading = signal(true);
  error = signal('');

  constructor(
    private projectService: ProjectService,
    private router: Router, // 🚀 Yönlendirme servisi eklendi
    private dialogRef: MatDialogRef<ProjectMembersDialog>, // 🚀 Dialog kapatma servisi eklendi
    @Inject(MAT_DIALOG_DATA) public data: { projectId: number }
  ) {}

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers(): void {
    this.projectService.getMembers(this.data.projectId).subscribe({
      next: (members) => {
        this.members.set(members);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  addMember(): void {
    if (!this.newEmail.trim()) return;
    this.error.set('');

    this.projectService.addMember(this.data.projectId, this.newEmail).subscribe({
      next: (members) => {
        this.members.set(members);
        this.newEmail = '';
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Üye eklenemedi');
      }
    });
  }

  removeMember(memberUserId: number): void {
    const confirmMessage = "Bu işlem geri alınamaz. Emin misiniz?";

    if (!confirm(confirmMessage)) {
      return;
    }

    this.projectService.removeMember(this.data.projectId, memberUserId).subscribe({
      next: (res: { message: string; isSelfRemoval: boolean; members: Member[] | null }) => {
        if (res.isSelfRemoval) {
          // 🚀 1. Kendi isteğiyle ayrıldıysa: Mesaj göster, modalı kapat ve projelere yönlendir
          alert(res.message);
          this.dialogRef.close(true);
          window.location.href = '/projects';
          this.router.navigate(['/projects']);
        } else {
          // 🚀 2. Başka bir üyeyi çıkardıysa: Üye listesini güncelle
          if (res.members) {
            this.members.set(res.members);
          }
        }
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Üye çıkarılamadı');
      }
    });
  }
}