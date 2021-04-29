import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BalanceSheetSelectorComponent } from './balance-sheet-selector.component';

describe('BalanceSheetSelectorComponent', () => {
  let component: BalanceSheetSelectorComponent;
  let fixture: ComponentFixture<BalanceSheetSelectorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BalanceSheetSelectorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BalanceSheetSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
