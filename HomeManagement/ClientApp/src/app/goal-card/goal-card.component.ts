import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { NgbModalConfig, NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'goal-card',
  templateUrl: './goal-card.component.html',
  styleUrls: ['./goal-card.component.css']
})
export class GoalCardComponent implements OnInit {
  @Input() financeSheet: any;
  @Input() sheetId: number;
  @Output() refreshEvent = new EventEmitter();
  @Output() errorEvent = new EventEmitter();

  constructor(private http: HttpClient, config: NgbModalConfig, private modalService: NgbModal) {
    config.backdrop = 'static';
    config.keyboard = false;
  }

  ngOnInit() {
  }

  onCreateGoal(newGoal) {
    let goal = { Name: newGoal.controls.name.value, Description: newGoal.controls.description.value, Amount: newGoal.controls.amount.value, EndDate: newGoal.controls.endDate.value, SheetId: this.sheetId }
    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
      'responseType': 'text' as 'json'
    };
    let json = JSON.stringify(goal);
    this.http.post("https://localhost:44352/api/sheet/" + this.sheetId + "/goal", JSON.stringify(json), httpOptions)
      .subscribe({
        next: (x) => { this.refreshEvent.emit(); },
        error: err => {
          this.errorEvent.emit(err.error);
        }
      });
  }

  onDeposit(deposit, id) {
    let amount = deposit.controls.amount.value;
    console.log(id);
    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
      'responseType': 'text' as 'json'
    };
    let json = JSON.stringify(amount);
    this.http.post("https://localhost:44352/api/sheet/" + this.sheetId + "/goal/" + id + "/deposit", JSON.stringify(json), httpOptions)
      .subscribe({
        next: (x) => { this.refreshEvent.emit(); },
        error: err => {
          console.log(err);
        }
      });
  }

  onGoalDelete(goalId) {
    this.http.delete("https://localhost:44352/api/sheet/" + this.sheetId + "/goal/" + goalId + "/delete").subscribe({
      next: (x) => { this.refreshEvent.emit(); },
      error: err => {
        console.log(err);
      }
    });
  }

  open(content) {
    this.modalService.open(content);
  }
}
