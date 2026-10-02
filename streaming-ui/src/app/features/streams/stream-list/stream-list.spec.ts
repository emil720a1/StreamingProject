import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { StreamList } from './stream-list';

describe('StreamList', () => {
  let component: StreamList;
  let fixture: ComponentFixture<StreamList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StreamList],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(StreamList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
