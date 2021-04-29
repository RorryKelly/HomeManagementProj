import { Component, OnInit, EventEmitter, Output, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { CookieService } from 'ngx-cookie-service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { NgbModal, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'expenditure-card',
  templateUrl: './expenditure-card.component.html',
  styleUrls: ['./expenditure-card.component.css']
})
export class ExpenditureCardComponent implements OnInit {
  @Input() financeSheet: any;
  @Input() sheetId: number;
  @Output() refreshEvent = new EventEmitter();
  @Output() errorEvent = new EventEmitter();

  expenditureTypeOption;
  expenditureReoccuranceOption;
  preempt = false;
  expenditureHovered = false;

  constructor(private route: ActivatedRoute, private cookieService: CookieService, private http: HttpClient, config: NgbModalConfig, private modalService: NgbModal) {
    config.backdrop = 'static';
    config.keyboard = false;
  }

  ngOnInit() {
    this.expenditureTypeOption = "Select A Type";
    this.expenditureReoccuranceOption = "Select A Reoccurance Type";
  }

  open(content) {
    this.modalService.open(content);
  }

  onCreateExpenditure(newExpenditure) {
    let incomeType;
    switch (this.expenditureTypeOption) {
      case "Select A Type":
        incomeType = -1;
        break;
      case "Other":
        incomeType = 0;
        break;
      case "Utility Bill":
        incomeType = 1;
        break;
      case "Shopping":
        incomeType = 2;
        break;
    }

    if (this.expenditureReoccuranceOption == 'One Time') {
      this.preempt = true;
    }

    let expenditure = { Name: newExpenditure.controls.name.value, Amount: newExpenditure.controls.amount.value, Type: incomeType, Reoccurance: this.expenditureReoccuranceOption, SheetId: this.sheetId, PayPreemptively: this.preempt };

    if (incomeType != -1 && this.expenditureReoccuranceOption != "Select A Reoccurance Type") {
      let httpOptions = {
        headers: new HttpHeaders({
          'Content-Type': 'application/json'
        }),
        'responseType': 'text' as 'json'
      };
      let json = JSON.stringify(expenditure);
      this.http.post("https://localhost:44352/api/sheet/" + this.sheetId + "/expenditure", JSON.stringify(json), httpOptions)
        .subscribe({
          next: (x) => { console.log(x); this.refresh(); },
          error: err => {
            this.errorEvent.emit(err.error);
          }
        });
    }
  }

  onEpenditureDelete(expenditureId) {
    this.http.delete("https://localhost:44352/api/sheet/" + this.sheetId + "/expenditure/" + expenditureId + "/delete").subscribe({
      next: (x) => { console.log(x); this.refresh(); },
      error: err => {
        console.log(err);
      }
    });
  }

  refresh() {
    this.expenditureTypeOption = "Select A Type";
    this.expenditureReoccuranceOption = "Select A Reoccurance Type";
    this.preempt = false;

    this.refreshEvent.emit();
  }
}
