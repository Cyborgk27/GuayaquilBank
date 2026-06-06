import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoicePdfViewer } from './invoice-pdf-viewer';

describe('InvoicePdfViewer', () => {
  let component: InvoicePdfViewer;
  let fixture: ComponentFixture<InvoicePdfViewer>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InvoicePdfViewer],
    }).compileComponents();

    fixture = TestBed.createComponent(InvoicePdfViewer);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
