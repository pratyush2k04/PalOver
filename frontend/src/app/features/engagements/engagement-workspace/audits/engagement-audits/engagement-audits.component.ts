import { Component, computed, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { AuditLogComponent } from '@features/audit-log/audit-log';

@Component({
  selector: 'app-engagement-audits',
  standalone: true,
  imports: [AuditLogComponent],
  templateUrl: './engagement-audits.component.html',
})
export class EngagementAuditsComponent {
  private route = inject(ActivatedRoute);

  private parentParamMap = toSignal(this.route.parent!.paramMap);

  engagementId = computed(() => {
    const idStr = this.parentParamMap()?.get('id');
    return idStr ? +idStr : null;
  });
}
