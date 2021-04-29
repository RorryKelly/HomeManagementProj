import { TestBed } from '@angular/core/testing';

import { ReoccuranceGetterService } from './reoccurance-getter.service';

describe('ReoccuranceGetterService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ReoccuranceGetterService = TestBed.get(ReoccuranceGetterService);
    expect(service).toBeTruthy();
  });
});
