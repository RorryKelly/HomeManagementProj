import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { CookieService } from 'ngx-cookie-service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { NgbModal, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'balance-sheet',
  templateUrl: './balance-sheet.component.html',
  styleUrls: ['./balance-sheet.component.css']
})
export class BalanceSheetComponent implements OnInit {

  constructor(private route: ActivatedRoute, private cookieService: CookieService, private http: HttpClient, config: NgbModalConfig, private modalService: NgbModal, private router: Router) {
    config.backdrop = 'static';
    config.keyboard = false;
  }

  ngOnInit() {
    this.typeOption = "Select A Type";
    this.reoccuranceOption = "Select A Reoccurance Type";
    this.expenditureTypeOption = "Select A Type";
    this.expenditureReoccuranceOption = "Select A Reoccurance Type";
    this.route.paramMap.subscribe(params => {
      this.sheetId = params.get('id');
      this.getSheetData(this.sheetId);
    })
  }

  sheetId
  financeSheet;
  errorMsg;
  incomeHovered = false;
  expenditureHovered = false;
  savingHovered = false;
  typeOption;
  reoccuranceOption;
  expenditureTypeOption;
  expenditureReoccuranceOption;
  preempt = false;
  searchedUsers;
  selectedUser;
  searchedUsersSelected = false;

  getSheetData(id) {
    let cookie = localStorage.getItem("login");
    const helper = new JwtHelperService();

    const decodedToken = helper.decodeToken(cookie);
    let userId = decodedToken.Id;
    this.http.get("https://localhost:44352/api/user/" + userId + "/sheet/" + id).subscribe({
      next: (x) => {
        this.financeSheet = x;
        console.log(this.financeSheet);
      },
      error: err => {
        if (err.status == 404)
          this.router.navigate(['/404']);
      }
    });
  }

  onUserSearch(create) {
    if (create.controls.ownerName.value != "") {
      this.http.get("https://localhost:44352/api/getUsers/" + create.controls.ownerName.value).subscribe({
        next: (x) => {
          console.log(x);
          this.searchedUsers = x;
          this.searchedUsersSelected = null;
        },
        error: err => {
          console.log(err)
        }
      });
    }
  }

  onAddOwner(newOwner) {
    console.log(this.selectedUser);

    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
      'responseType': 'text' as 'json'
    };

    this.http.post("https://localhost:44352/api/sheet/" + this.sheetId + "/owners/add/" + this.selectedUser.id, httpOptions).subscribe({
      next: (x) => {
        console.log(x);
        this.searchedUsers = x;
        this.searchedUsersSelected = null;
      },
      error: err => {
        console.log(err)
      }
    });
  }

  onReportCreation() {
    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
      'responseType': 'text' as 'json'
    };

    let json = JSON.stringify(this.financeSheet)

    this.http.post("https://localhost:44352/api/report", JSON.stringify(json), httpOptions).subscribe({
      next: (x) => {
        console.log(x);
        this.router.navigate(['/report/' + x]);
      },
      error: err => {
        console.log(err)
      }
    });
  }

  refresh() {
    this.typeOption = "Select A Type";
    this.reoccuranceOption = "Select A Reoccurance Type";
    this.expenditureTypeOption = "Select A Type";
    this.expenditureReoccuranceOption = "Select A Reoccurance Type";
    this.getSheetData(this.sheetId);
  }

  open(content) {
    this.modalService.open(content);
  }
}
