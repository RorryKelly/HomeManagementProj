import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.css']
})
export class ReportComponent implements OnInit {
  incomeData = [];
  expenditureData = [];
  expenditureColumns = [];
  goalData = [];
  reportId;
  report;
  dateCompiled;
  reportRating;
  expenditureOptions = {
    isStacked: true
  }

  constructor(private route: ActivatedRoute, private http: HttpClient) {
  }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.reportId = params.get('id');
      this.getData();
    })

  }

  getData() {
    this.http.get("https://localhost:44352/api/report/" + this.reportId).subscribe({
      next: (x) => {
        console.log(x);
        this.report = x;
        this.formatIncomeData();
        this.dateCompiled = this.report.dateCreated.split("T")[0];
      },
      error: err => {
        console.log(err);
      }
    });
  }

  formatIncomeData() {
    let totalAnual = 0;
    let formatedIncomes = [];
    let expenditureBar = [];
    let incomeBar = [];
    let columnNames = [];

    for (let i = 0; i <= this.report.incomes.length - 1; i++) {
      formatedIncomes.push([this.report.incomes[i].name, this.report.incomes[i].annualAmount])
      totalAnual += this.report.incomes[i].annualAmount;
    }
    this.incomeData = formatedIncomes;

    columnNames.push('names');
    columnNames.push('Total Income (£)');
    incomeBar.push('Annual Income');
    expenditureBar.push('Total Expenditures');
    incomeBar.push(totalAnual);
    expenditureBar.push(0);

    for (let i = 0; i <= this.report.expenditures.length - 1; i++) {
      expenditureBar.push(this.report.expenditures[i].annualAmount)
      columnNames.push(this.report.expenditures[i].name)
      incomeBar.push(0);
    }

    this.expenditureColumns = columnNames;
    this.expenditureData = [incomeBar, expenditureBar] 

    let formattedGoals = [];
    for (let i = 0; i <= this.report.savingGoals.length - 1; i++) {
      formattedGoals.push([this.report.savingGoals[i].name, (this.report.savingGoals[i].deposited / this.report.savingGoals[i].amount) * 100]);
    }
    this.goalData = formattedGoals;

    console.log(this.expenditureData);
  }
}
