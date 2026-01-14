import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SpaceEditor } from './space-editor';

describe('SpaceEditor', () => {
  let component: SpaceEditor;
  let fixture: ComponentFixture<SpaceEditor>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SpaceEditor]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SpaceEditor);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
