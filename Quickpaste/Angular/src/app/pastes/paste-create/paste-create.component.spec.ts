import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PasteCreateComponent } from './paste-create.component';

describe('PasteCreateComponent', () => {
  let component: PasteCreateComponent;
  let fixture: ComponentFixture<PasteCreateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PasteCreateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PasteCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
