import { TestBed } from '@angular/core/testing';

import { StoreRoutingService } from './store-routing.module';

describe('StoreRoutingService', () => {
  let service: StoreRoutingService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StoreRoutingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
