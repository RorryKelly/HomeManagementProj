import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ReoccuranceGetterService {

  constructor() { }

  get(reoccurances) {
    let reoccuranceStr = [];
    for (let reoccurance in reoccurances) {
      switch (reoccurances) {
        case 1:
          reoccuranceStr.push("One Time");
          break;
        case 2:
          reoccuranceStr.push("Hourly");
          break;
        case 3:
          reoccuranceStr.push("Bi-Daily");
          break;
        case 4:
          reoccuranceStr.push("Daily");
          break;
        case 5:
          reoccuranceStr.push("Bi-Weekly");
          break;
        case 6:
          reoccuranceStr.push("Weekly");
          break;
        case 7:
          reoccuranceStr.push("Bi-Monthly");
          break;
        case 8:
          reoccuranceStr.push("Monthly");
          break;
        case 9:
          reoccuranceStr.push("Bi-Anually");
          break;
        case 10:
          reoccuranceStr.push("Anually");
          break;
      }
      return reoccuranceStr;
    }
  }
}

class income {

}
