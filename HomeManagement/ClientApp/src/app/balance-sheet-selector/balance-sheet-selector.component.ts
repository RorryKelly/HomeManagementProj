import { Component, OnInit } from '@angular/core';
import { NgbModalConfig, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Router } from '@angular/router';


@Component({
  selector: 'app-balance-sheet-selector',
  templateUrl: './balance-sheet-selector.component.html',
  styleUrls: ['./balance-sheet-selector.component.css'],
  providers: [NgbModalConfig, NgbModal]
})
export class BalanceSheetSelectorComponent implements OnInit {

  constructor(config: NgbModalConfig, private modalService: NgbModal, private cookieService: CookieService, private http: HttpClient, public router: Router) {
    config.backdrop = 'static';
    config.keyboard = false;
  }

  userId;
  sheetCreateError;
  financeSheets;
  searchedUsers;
  searchedUsersSelected;
  selectedUsers = [];


  ngOnInit() {
    let cookie = localStorage.getItem("login");
    const helper = new JwtHelperService();
    this.searchedUsersSelected = true;

    const decodedToken = helper.decodeToken(cookie);
    this.userId = decodedToken.Id;
    this.getSheets();
  }

  open(content) {
    this.modalService.open(content);
  }

  onCreate(create) {
    let selectedUserIds = [];
    selectedUserIds.push(this.userId);
    this.selectedUsers.forEach(user => selectedUserIds.push(user.id));
    let financeSheet = { Name: create.controls.name.value, Balance: create.controls.balance.value, Owners: selectedUserIds }
    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
      'responseType': 'text' as 'json'
    };
    let json = JSON.stringify(financeSheet)
    this.http.post("https://localhost:44352/api/FinanceSheet", JSON.stringify(json), httpOptions)
      .subscribe({
        next: (x) => this.router.navigate(['/financesheet/' + x]),
        error: err => {
          this.sheetCreateError = err.error;
        }
      });
  }

  onUserSearch(create) {
    if (create.controls.share.value != "") {
      this.http.get("https://localhost:44352/api/getUsers/" + create.controls.share.value).subscribe({
        next: (x) => {
          console.log(x);
          this.searchedUsers = x;
          this.searchedUsersSelected = false;
        },
        error: err => {
          console.log(err)
        }
      });
    }
  }

  onUserClick(user) {
    this.searchedUsersSelected = true;
    this.selectedUsers.push(user);
    console.log(user);
  }

  removeUser(user) {
    let pos = this.selectedUsers.indexOf(user);
    this.selectedUsers.splice(pos, 1);
  }

  onDelete(id) {
    this.http.delete("https://localhost:44352/api/sheet/" + id + "/delete").subscribe({
      next: (x) => {
        this.getSheets();
      },
      error: err => {
        console.log(err)
      }
    });
  }

  getSheets() {
    this.http.get("https://localhost:44352/api/UsersFinanceSheets/" + this.userId).subscribe({
      next: (x) => this.financeSheets = x,
      error: err => {
        console.log(err)
      }
    });
  }
}
