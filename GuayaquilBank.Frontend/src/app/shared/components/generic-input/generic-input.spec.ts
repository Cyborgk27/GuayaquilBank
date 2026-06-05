import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GenericInput } from './generic-input';

describe('GenericInput', () => {
  let component: GenericInput;
  let fixture: ComponentFixture<GenericInput>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GenericInput],
    }).compileComponents();

    fixture = TestBed.createComponent(GenericInput);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
