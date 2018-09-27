import * as models from './models'

export function isModifingState(appStatus: models.AppStatus): boolean {
	return appStatus.isCreatingMessage || appStatus.isEditing;
}
