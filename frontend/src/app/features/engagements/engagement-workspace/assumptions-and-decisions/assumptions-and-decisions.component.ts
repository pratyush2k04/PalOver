import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { AssumptionsComponent } from './components/assumptions/assumptions.component';
import { DependenciesComponent } from './components/dependencies/dependencies.component';
import { AssumptionService } from '../../services/assumption.service';
import { AssumptionStatus } from '../../models/assumption.model';
import { DependencyService } from '../../services/dependency.service';
import { DependencyStatus } from '../../models/dependency.model';

import { InlineSVGModule } from 'ng-inline-svg-2';

@Component({
  selector: 'app-assumptions-and-decisions',
  standalone: true,
  imports: [CommonModule, AssumptionsComponent, DependenciesComponent, InlineSVGModule],
  templateUrl: './assumptions-and-decisions.component.html',
  styleUrl: './assumptions-and-decisions.component.scss'
})
export class AssumptionsAndDecisionsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private assumptionService = inject(AssumptionService);
  private dependencyService = inject(DependencyService);

  engagementId = signal<number>(0);
  invalidAssumptionsCount = signal<number>(0);
  unvalidatedAssumptionsCount = signal<number>(0);
  totalAssumptionsCount = signal<number>(0);

  ngOnInit() {
    this.route.parent?.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.engagementId.set(+id);
      }
    });
  }
}
