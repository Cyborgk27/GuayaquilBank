import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GenericModal } from './generic-modal';

describe('GenericModal', () => {
  let component: GenericModal;
  let fixture: ComponentFixture<GenericModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GenericModal],
    }).compileComponents();

    fixture = TestBed.createComponent(GenericModal);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
