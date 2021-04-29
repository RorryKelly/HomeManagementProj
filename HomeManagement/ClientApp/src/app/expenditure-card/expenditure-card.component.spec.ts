import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpenditureCardComponent } from './expenditure-card.component';

describe('ExpenditureCardComponent', () => {
  let component: ExpenditureCardComponent;
  let fixture: ComponentFixture<ExpenditureCardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExpenditureCardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExpenditureCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
