import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectMembersDialog } from './project-members-dialog';

describe('ProjectMembersDialog', () => {
  let component: ProjectMembersDialog;
  let fixture: ComponentFixture<ProjectMembersDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProjectMembersDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(ProjectMembersDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
