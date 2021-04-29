import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit() {
  }

  register(reg) {
    console.log(reg);
    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    }
    let json = JSON.stringify(reg);
    this.http.post("https://localhost:44352/api/register", JSON.stringify(json), httpOptions)
      .subscribe({
        next: (x) => {
          this.router.navigate(['/'])
        },
        error: err => {
          console.log(err)
        }
      });
  }

}
