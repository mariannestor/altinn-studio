import React from 'react';
import { useParams } from 'react-router-dom';
import { DataModelling } from 'app-shared/features';
import { connect } from 'react-redux';
import { DataModelsMetadataActions } from 'app-shared/features/dataModelling/sagas/metadata';
import { createStyles, Grid, withStyles } from '@material-ui/core';
import { useAppDispatch } from 'common/hooks';
import type { IDashboardAppState } from '../../types/global';

interface IStandaloneDataModellingProps {
  language: any;
  classes: any;
}

const styles = createStyles({
  containerGrid: {
    marginTop: 70,
  },
});

const DataModellingContainer = ({
  classes,
  language,
}: IStandaloneDataModellingProps) => {
  const dispatch = useAppDispatch();
  dispatch(DataModelsMetadataActions.getDataModelsMetadata());

  const { org, repoName } = useParams();
  if( org && repoName) {
    return (
      <Grid item className={classes.containerGrid}>
        <DataModelling
          language={language}
          org={org}
          repo={repoName}
          createPathOption
        />
      </Grid>
    );
  }
  return <p>Either organization/repository-name was undefined</p>
};

const mapStateToProps = (
  state: IDashboardAppState,
  props: IStandaloneDataModellingProps,
) => {
  return {
    classes: props.classes,
    language: state.language.language,
  };
};
const standaloneDataModelling = connect(mapStateToProps)(
  DataModellingContainer,
);
export default withStyles(styles)(standaloneDataModelling);
