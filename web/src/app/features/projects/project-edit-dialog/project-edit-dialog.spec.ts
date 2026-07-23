import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectEditDialog } from './project-edit-dialog';

describe('ProjectEditDialog', () => {
  let component: ProjectEditDialog;
  let fixture: ComponentFixture<ProjectEditDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProjectEditDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(ProjectEditDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
