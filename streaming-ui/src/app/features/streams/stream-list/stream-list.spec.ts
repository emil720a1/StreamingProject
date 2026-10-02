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

  it('should return all categories', () => {
    expect(component.categories).toEqual([
      'All',
      'Gaming',
      'Music',
      'Technology',
    ]);
  });

  it('should show all streams by default', () => {
    expect(component.filteredStreams.length).toBe(3);
  });

  it('should filter streams by category', () => {
    component.selectCategory('Gaming');

    expect(component.filteredStreams.length).toBe(1);
    expect(component.filteredStreams[0].title).toBe('Gaming Live');
  });

  it('should show all streams after selecting All', () => {
    component.selectCategory('Gaming');
    component.selectCategory('All');

    expect(component.filteredStreams.length).toBe(3);
  });
});
