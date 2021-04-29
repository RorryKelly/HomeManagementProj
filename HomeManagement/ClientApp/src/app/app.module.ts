import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { GoogleChartsModule } from 'angular-google-charts';



import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { InitComponent } from './init/init.component';
import { BalanceSheetSelectorComponent } from './balance-sheet-selector/balance-sheet-selector.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { BalanceSheetComponent } from './balance-sheet/balance-sheet.component';
import { IncomeCardComponent } from './income-card/income-card.component';
import { ExpenditureCardComponent } from './expenditure-card/expenditure-card.component';
import { GoalCardComponent } from './goal-card/goal-card.component';
import { ReportComponent } from './report/report.component';
import { NotFoundPageComponent } from './not-found-page/not-found-page.component';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    LoginComponent,
    RegisterComponent,
    InitComponent,
    BalanceSheetSelectorComponent,
    BalanceSheetComponent,
    IncomeCardComponent,
    ExpenditureCardComponent,
    GoalCardComponent,
    ReportComponent,
    NotFoundPageComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: InitComponent },
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'home', component: BalanceSheetSelectorComponent },
      { path: 'financesheet/:id', component: BalanceSheetComponent },
      { path: 'report/:id', component: ReportComponent },
      { path: '404', component: NotFoundPageComponent }
    ]),
    NgbModule,
    GoogleChartsModule
  ],
  providers: [CookieService],
  bootstrap: [AppComponent]
})
export class AppModule { }
