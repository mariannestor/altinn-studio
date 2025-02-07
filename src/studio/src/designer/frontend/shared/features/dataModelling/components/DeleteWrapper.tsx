import { TopToolbarButton } from '@altinn/schema-editor/index';
import React from 'react';
import { getLanguageFromKey } from '../../../utils/language';
import DeleteDialog from './DeleteDialog';
import {makeStyles} from "@material-ui/core";
import theme from "app-shared/theme/altinnStudioTheme";

const useStyles = makeStyles({
  root: {
    '&:not(:hover):not(:disabled)': {
      color: theme.altinnPalette.primary.red + ' !important'
    }
  }
});

export interface IDeleteWrapper {
  language: any;
  deleteAction: () => void;
  schemaName: string;
}

export default function DeleteWrapper(props: IDeleteWrapper) {
  const [deleteButtonAnchor, setDeleteButtonAnchor] = React.useState(null);
  const onDeleteClick = (event: any) => {
    setDeleteButtonAnchor(event.currentTarget);
  };
  const onDeleteConfirmClick = () => {
    props.deleteAction();
    setDeleteButtonAnchor(null);
  };
  const onCancelDelete = () => {
    setDeleteButtonAnchor(null);
  };
  const classes = useStyles();

  return (
    <>
      <TopToolbarButton
        id='delete-model-button'
        disabled={!props.schemaName}
        faIcon='ai ai-trash'
        iconSize={24}
        onClick={onDeleteClick}
        warning
        className={classes.root}
      >
        {getLanguageFromKey('general.delete', props.language)}
      </TopToolbarButton>
      {deleteButtonAnchor && (
        <DeleteDialog
          anchor={deleteButtonAnchor}
          language={props.language}
          schemaName={props.schemaName}
          onConfirm={onDeleteConfirmClick}
          onCancel={onCancelDelete}
        />
      )}
    </>
  );
}
