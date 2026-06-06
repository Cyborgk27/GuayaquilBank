import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GenericAutocomplete } from './generic-autocomplete';

describe('GenericAutocomplete', () => {
  let component: GenericAutocomplete;
  let fixture: ComponentFixture<GenericAutocomplete>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GenericAutocomplete],
    }).compileComponents();

    fixture = TestBed.createComponent(GenericAutocomplete);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
