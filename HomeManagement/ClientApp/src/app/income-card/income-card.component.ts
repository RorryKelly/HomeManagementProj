import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { NgbModalConfig, NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'income-card',
  templateUrl: './income-card.component.html',
  styleUrls: ['./income-card.component.css']
})
export class IncomeCardComponent implements OnInit {
  @Input() financeSheet: any;
  @Input() sheetId: number;
  @Output() refreshEvent = new EventEmitter();
  @Output() errorEvent = new EventEmitter();

  incomeHovered = false;
  typeOption;
  reoccuranceOption;
  preempt = false;

  constructor(private http: HttpClient, config: NgbModalConfig, private modalService: NgbModal) {
    config.backdrop = 'static';
    config.keyboard = false;
  }

  ngOnInit() {
    this.typeOption = "Select A Type";
    this.reoccuranceOption = "Select A Reoccurance Type";
  }

  onCreateIncome(newIncome) {
    let incomeType;
    switch (this.typeOption) {
      case "Select A Type":
        incomeType = -1;
        break;
      case "Other":
        incomeType = 0;
        break;
      case "Work":
        incomeType = 1;
        break;
      case "Dividend":
        incomeType = 2;
        break;
    }

    if (this.reoccuranceOption == 'One Time') {
      this.preempt = true;
    }

    let income = { Name: newIncome.controls.name.value, Amount: newIncome.controls.amount.value, Type: incomeType, Reoccurance: this.reoccuranceOption, SheetId: this.sheetId, PayPreemptively: this.preempt };

    if (incomeType != -1 && this.reoccuranceOption != "Select A Reoccurance Type") {
      let httpOptions = {
        headers: new HttpHeaders({
          'Content-Type': 'application/json'
        }),
        'responseType': 'text' as 'json'
      };
      let json = JSON.stringify(income);
      this.http.post("https://localhost:44352/api/sheet/" + this.sheetId + "/income", JSON.stringify(json), httpOptions)
        .subscribe({
          next: (x) => { this.refresh(); },
          error: err => {
            this.errorEvent.emit(err.error);
          }
        });
    }
  }

  onIncomeDelete(incomeId) {
    this.http.delete("https://localhost:44352/api/sheet/" + this.sheetId + "/income/" + incomeId + "/delete").subscribe({
      next: (x) => { this.refresh(); },
      error: err => {
        console.log(err);
      }
    });
  }

  refresh() {
    this.typeOption = "Select A Type";
    this.reoccuranceOption = "Select A Reoccurance Type";
    this.preempt = false;

    this.refreshEvent.emit();
  }

  open(content) {
    this.modalService.open(content);
  }
}
