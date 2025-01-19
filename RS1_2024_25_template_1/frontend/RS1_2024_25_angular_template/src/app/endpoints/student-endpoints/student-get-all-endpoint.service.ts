import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {MyPagedRequest} from '../../helper/my-paged-request';
import {MyConfig} from '../../my-config';
import {buildHttpParams} from '../../helper/http-params.helper';
import {MyBaseEndpointAsync} from '../../helper/my-base-endpoint-async.interface';
import {MyPagedList} from '../../helper/my-paged-list';
import {MyCacheService} from '../../services/cache-service/my-cache.service';
import {of} from 'rxjs';
import {tap} from 'rxjs/operators';

// DTO za zahtjev
export interface StudentGetAllRequest extends MyPagedRequest {
  q?: string; // Upit za pretragu (ime, prezime, broj indeksa, itd.)
  firstLast?: string;
}

// DTO za odgovor
export interface StudentGetAllResponse {
  id: number;
  firstName: string;
  lastName: string;
  studentNumber: string;
  citizenship?: string; // Državljanstvo
  birthMunicipality?: string; // Općina rođenja
  isDeleted:boolean;
}

@Injectable({
  providedIn: 'root',
})
export class StudentGetAllEndpointService
  implements MyBaseEndpointAsync<StudentGetAllRequest, MyPagedList<StudentGetAllResponse>> {
  private apiUrl = `${MyConfig.api_address}/students/filter`;

  constructor(private httpClient: HttpClient, private cacheService: MyCacheService) {
  }

  handleAsync(request: StudentGetAllRequest,useCache: boolean = false, cacheTTL: number = 300000) {
    let key=`students--${request.pageNumber || 1}-${request.pageSize}-${request.firstLast}`;
    if(key){
      let hit=this.cacheService.get<MyPagedList<StudentGetAllResponse>>(key);
      if(hit){
        console.log("Keširano1");
        return of(hit);
      }
    }
    const params = buildHttpParams(request); // Pretvori DTO u query parametre

    return this.httpClient.get<MyPagedList<StudentGetAllResponse>>(`${this.apiUrl}`, {params}).pipe(
      tap((data) => {
        if(useCache){
          this.cacheService.set(key, data, cacheTTL);
          console.log("Keširano 2");
        }
      })
    )
  }
}
