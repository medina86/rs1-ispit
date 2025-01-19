import {Component, OnInit} from '@angular/core';
import {
  CountryGetAllEndpointService,
  CountryGetAllResponse
} from '../../../../endpoints/country-endpoints/country-get-all-endpoint.service';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {HttpClient} from '@angular/common/http';
import {ActivatedRoute, Router} from '@angular/router';
import {MyConfig} from '../../../../my-config';
import {min} from 'rxjs';

export interface GetStudentByID{
  id: number;
  firstName: string;
  lastName: string;
  studentNumber: string;
  birthMunicipalityId: number;
  birthMunicipalityName?: string;
  phone: string;
  birthDate: string;
  countryId: number;
}
export interface Municipalities{
  id: number;
  name: string;
}

@Component({
  selector: 'app-student-update',
  standalone: false,

  templateUrl: './student-update.component.html',
  styleUrl: './student-update.component.css'
})
export class StudentUpdateComponent implements  OnInit {
  municipalities: Municipalities[] = [];
  countries: CountryGetAllResponse[]=[];
  student: GetStudentByID| null=null;

  form=new FormGroup({
    phone:new FormControl('', Validators.pattern('^06\d-\d\d\d-\d\d\d$')),
    birthDate:new FormControl('', Validators.required),
    bdValidator: new FormControl(Date.now(), [Validators.min(new Date('1950-01.01').getTime()),Validators.max(new Date('2025-01.01').getTime()) ]),
    countryId:new FormControl(0, Validators.required),
    municipalityId: new FormControl(0, Validators.required),
  })

  constructor(private countriesAll: CountryGetAllEndpointService,
              private http: HttpClient,
              private router: Router,
              private route: ActivatedRoute) {
  }

  ngOnInit() {
    this.form.get('countryId')?.valueChanges.subscribe({
      next: data => {
        this.loadMunicipalities(data!);
      }
    })

    this.form.get('birthDate')?.valueChanges.subscribe(
      data=>{
        this.form.get('bdValidator')!.setValue(new Date(data!).getTime());
      }
    )
    this.route.params.subscribe(params => {
      let id = Number(params['id']);
      if(id){
        this.http.get<GetStudentByID>(`${MyConfig.api_address}/students/id/${id}`).subscribe({
          next: data => {
            this.student = data;
            console.log(this.student);
            this.form.patchValue({
              birthDate: data.birthDate,
              phone: data.phone,
              municipalityId: data.birthMunicipalityId,
              countryId: data.countryId,
            })
          }
        })
      }
    })
    this.loadCountries();
  }

  private loadCountries() {
    this.countriesAll.handleAsync().subscribe(
      data=>{
        this.countries = data;
      }
    )
  }

  private loadMunicipalities(countryId:number) {
    this.http.get<Municipalities[]>(`${MyConfig.api_address}/municiplaities/all/${countryId}`).subscribe({
      next: (data)=> {
        this.municipalities=data!;
      }
    })
  }

  updateStudent() {
    let std={
      Id: this.student?.id!,
      Phone: this.form.get('phone')?.value!,
      BirthDate: this.form.get('birthDate')?.value!,
      MunicipalityId: this.form.get('municipalityId')?.value!
    }
    console.log(std);

    this.http.put<GetStudentByID>(`${MyConfig.api_address}/students/update`, std).subscribe({
      next: (data)=>{
        this.student=data!
        console.log(this.student);
        alert("Successfully updated");
      }
    })
  }
}
