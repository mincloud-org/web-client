import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StorageSelector } from './storage-selector';

describe('StorageSelector', () => {
  let component: StorageSelector;
  let fixture: ComponentFixture<StorageSelector>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StorageSelector]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StorageSelector);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
