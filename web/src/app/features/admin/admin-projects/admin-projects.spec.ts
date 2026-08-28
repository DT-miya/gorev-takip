import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminProjects } from './admin-projects';

describe('AdminProjects', () => {
  let component: AdminProjects;
  let fixture: ComponentFixture<AdminProjects>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminProjects],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminProjects);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
