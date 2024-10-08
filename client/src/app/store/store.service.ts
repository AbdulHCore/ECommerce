import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IPagination } from '../shared/models/pagination';
import { IProduct } from '../shared/models/product';
import { IBrand } from '../shared/models/brand';
import { IType } from '../shared/models/type';
import { StoreParams } from '../shared/models/storeparams';

@Injectable({
  providedIn: 'root'
})
export class StoreService {

  constructor(private httpClient: HttpClient) { 

  }
  baseUrl='http://localhost:8010/';

  getProducts(storeParams: StoreParams){
      let params = new HttpParams();
      if(storeParams.brandId){
        params = params.append('brandId', storeParams.brandId);
      }
      if(storeParams.typeId){
        params = params.append('typeId', storeParams.brandId);
      }

    return this.httpClient.get<IPagination<IProduct[]>>(this.baseUrl+'Catalog/GetAllProducts', {params})
  }

  getBrands(){
    return this.httpClient.get<IBrand[]>(this.baseUrl+'Catalog/GetAllBrands');
  }

  getTypes(){
    return this.httpClient.get<IType[]>(this.baseUrl+'Catalog/GetProductType');
  }
}
