import {Component, OnInit} from '@angular/core';
import {GetStudentByID} from '../student-update/student-update.component';
import {MatTableDataSource} from '@angular/material/table';
import {HttpClient} from '@angular/common/http';
import {ActivatedRoute, Router} from '@angular/router';
import {MyAuthService} from '../../../../services/auth-services/my-auth.service';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {MyConfig} from '../../../../my-config';

export interface YOSGetResponse {
  id: number
  akademskaGodina: number;
  godinaStudija: number;
  obnova: boolean;
  datumUpisa: string;
  snimio: string;
}

export interface AcademicYearGetResponse {
  id: number,
  name: string,
  startDate: string,
  endDate: string
}
@Component({
  selector: 'app-semesters',
  standalone: false,

  templateUrl: './semesters.component.html',
  styleUrl: './semesters.component.css'
})
export class SemestersComponent implements OnInit{
  student:GetStudentByID|null=null;
  studyYears: YOSGetResponse[]=[];
  academicYears: AcademicYearGetResponse[]=[];
  createNew=true;

  dataSource=new MatTableDataSource<YOSGetResponse>();
  displayedColumns= ["id", "akademska", "godinaStudija", "obnova", "datumUpisa", "snimio"];

  form = new FormGroup({
    studentId: new FormControl(0, [Validators.required]),
    datumUpisa: new FormControl('', [Validators.required]),
    godinaStudija: new FormControl<number>(0,[Validators.required, Validators.min(1), Validators.max(5)]),
    akademskaGodinaId: new FormControl(1, [Validators.required]),
    obnova: new FormControl(false),
    cijenaSkolarine: new FormControl(0),
  })

  constructor(private http: HttpClient,
              private route: ActivatedRoute,
              private router: Router,
              private auth: MyAuthService) {
  }

  ngOnInit(){
    this.form.get('obnova')!.disable();
    this.form.get('cijenaSkolarine')!.disable();

    this.http.get<AcademicYearGetResponse[]>(`${MyConfig.api_address}/years/academic`).subscribe(
      data=>{
        this.academicYears=data;
        this.form.get('akademskaGodinaId')!.setValue(this.academicYears[1].id);
      })
    this.form.get('datumUpisa')!.valueChanges.subscribe(data => {});
    this.form.get('godinaStudija')!.valueChanges.subscribe({
      next:data=>{
        if((data!<1 || data!>5)&& data! != null){
          this.form.get('godinaStudija')!.setValue(1);
        }
        let obnova= this.studyYears.find(a=>a.godinaStudija===data!)!=undefined;

        this.form.get('obnova')?.setValue(obnova);
        this.form.get('cijenaSkolarine')!.setValue(obnova? 400:1800);
      }
    });


    this.route.params.subscribe(params => {
      let id = params['id'];
      if(id)
      {
        this.http.get<GetStudentByID>(`${MyConfig.api_address}/students/id/${id}`).subscribe({
          next: data => {
            this.student = data;
            console.log(data);

            this.http.get<YOSGetResponse[]>(`${MyConfig.api_address}/years/all/${id}`).subscribe({
              next: data => {
                this.studyYears = data;
                this.dataSource.data = this.studyYears;
                this.form.get('godinaStudija')!.setValue(1);

                console.log(this.studyYears);
              }
            })
          }
        })
      }})
  }

  createYos(){
    let y = {
      studentId: this.student!.id,
      datumUpisa: new Date(Date.now()),
      godinaStudija: this.form.get('godinaStudija')!.value,
      akademskaGodinaId: this.form.get('akademskaGodinaId')!.value,
      snimioId: this.auth.getMyAuthInfo()!.userId
    }

    console.log(y);
    this.http.post<YOSGetResponse>(`${MyConfig.api_address}/years/insert`, y).subscribe({
      next: val => {
        this.studyYears.push(val);
        this.dataSource.data = this.studyYears;
        this.form.get('godinaStudija')!.setValue(1);
      },
      error: (error) => { console.error('Error creating yos'); }
    })
  }

  showHideCreate() {
    this.createNew=!this.createNew;
  }

}
