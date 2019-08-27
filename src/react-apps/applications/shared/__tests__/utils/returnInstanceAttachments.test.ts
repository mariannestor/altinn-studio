// tslint:disable: max-line-length
import 'jest';
import returnInstanceAttachments from '../../src/utils/returnInstanceAttachments';

test('returnInstanceAttachments() returns correct attachment array', () => {
  const instance = {
      id:  '50001/c1572504-9fb6-4829-9652-3ca9c82dabb9',
      instanceOwnerId:  '50001',
      selfLinks: {
        apps:  'http://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9',
      },
      appId:  'matsgm/tjeneste-190814-1426',
      org:  'matsgm',
      createdDateTime:  '2019-08-22T15:38:15.1437757Z',
      createdBy:  '0',
      lastChangedDateTime:  '2019-08-22T15:38:15.1440262Z',
      lastChangedBy:  '0',
      process: {
        currentTask:  'Archived',
        isComplete:  true,
      },
      instanceState: {
        isDeleted:  false,
        isMarkedForHardDelete:  false,
        isArchived:  true,
      },
      data: [
        {
          id:  '585b2f4e-5ecb-417b-9d01-82b6e889e1d1',
          elementType:  'default',
          fileName:  '585b2f4e-5ecb-417b-9d01-82b6e889e1d1.xml',
          contentType:  'application/Xml',
          storageUrl:  'tjeneste-190814-1426/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/585b2f4e-5ecb-417b-9d01-82b6e889e1d1',
          dataLinks: {
            apps:  'http://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/585b2f4e-5ecb-417b-9d01-82b6e889e1d1',
          },
          fileSize:  0,
          isLocked:  false,
          createdDateTime:  '2019-08-22T15:38:15.1480698Z',
          createdBy:  '50001',
          lastChangedDateTime:  '2019-08-22T15:38:15.14807Z',
          lastChangedBy:  '50001',
        },
        {
          id:  '03e06136-88be-4866-a216-7959afe46137',
          elementType:  'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
          fileName:  '4mb.txt',
          contentType:  'text/plain',
          storageUrl:  'tjeneste-190814-1426/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/03e06136-88be-4866-a216-7959afe46137',
          dataLinks: {
            apps:  'http://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/03e06136-88be-4866-a216-7959afe46137',
          },
          fileSize:  4194304,
          isLocked:  false,
          createdDateTime:  '2019-08-22T15:38:27.4719761Z',
          createdBy:  '50001',
          lastChangedDateTime:  '2019-08-22T15:38:27.4719776Z',
          lastChangedBy:  '50001',
        },
        {
          id:  '11943e38-9fc4-43f6-84c4-12e529eebd28',
          elementType:  'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
          fileName:  '8mb.txt',
          contentType:  'text/plain',
          storageUrl:  'tjeneste-190814-1426/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/11943e38-9fc4-43f6-84c4-12e529eebd28',
          dataLinks: {
            apps:  'http://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/11943e38-9fc4-43f6-84c4-12e529eebd28',
          },
          fileSize:  8388608,
          isLocked:  false,
          createdDateTime:  '2019-08-22T15:38:28.0099729Z',
          createdBy:  '50001',
          lastChangedDateTime:  '2019-08-22T15:38:28.0099731Z',
          lastChangedBy:  '50001',
        },
        {
          id:  '092f032d-f54f-49c1-ae42-ebc0d10a2fcb',
          elementType:  'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
          fileName:  '2mb.txt',
          contentType:  'text/plain',
          storageUrl:  'tjeneste-190814-1426/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/092f032d-f54f-49c1-ae42-ebc0d10a2fcb',
          dataLinks: {
            apps:  'http://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/092f032d-f54f-49c1-ae42-ebc0d10a2fcb',
          },
          fileSize:  2097152,
          isLocked:  false,
          createdDateTime:  '2019-08-22T15:38:30.3266993Z',
          createdBy:  '50001',
          lastChangedDateTime:  '2019-08-22T15:38:30.3266995Z',
          lastChangedBy:  '50001',
        },
        {
          id:  '8698103b-fad1-4665-85c6-bf88a75ad708',
          elementType:  'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
          fileName:  '4mb.txt',
          contentType:  'text/plain',
          storageUrl:  'tjeneste-190814-1426/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/8698103b-fad1-4665-85c6-bf88a75ad708',
          dataLinks: {
            apps:  'http://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/8698103b-fad1-4665-85c6-bf88a75ad708',
          },
          fileSize:  4194304,
          isLocked:  false,
          createdDateTime:  '2019-08-22T15:38:44.2017248Z',
          createdBy:  '50001',
          lastChangedDateTime:  '2019-08-22T15:38:44.2017252Z',
          lastChangedBy:  '50001',
        },
        {
          id:  'e950864d-e304-41ca-a60c-0c5019166df8',
          elementType:  'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
          fileName:  '8mb.txt',
          contentType:  'text/plain',
          storageUrl:  'tjeneste-190814-1426/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/e950864d-e304-41ca-a60c-0c5019166df8',
          dataLinks: {
            apps:  'http://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/e950864d-e304-41ca-a60c-0c5019166df8',
          },
          fileSize:  8388608,
          isLocked:  false,
          createdDateTime:  '2019-08-22T15:38:44.6846318Z',
          createdBy:  '50001',
          lastChangedDateTime:  '2019-08-22T15:38:44.684632Z',
          lastChangedBy:  '50001',
        },
        {
          id:  '005d5bc3-a315-4705-9b06-3788fed86da1',
          elementType:  'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
          fileName:  '2mb.txt',
          contentType:  'text/plain',
          storageUrl:  'tjeneste-190814-1426/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/005d5bc3-a315-4705-9b06-3788fed86da1',
          dataLinks: {
            apps:  'http://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/005d5bc3-a315-4705-9b06-3788fed86da1',
          },
          fileSize:  2097152,
          isLocked:  false,
          createdDateTime:  '2019-08-22T15:38:46.8968953Z',
          createdBy:  '50001',
          lastChangedDateTime:  '2019-08-22T15:38:46.8968955Z',
          lastChangedBy:  '50001',
        },
      ],
  };

  const attachmentsTestData = [
    {
      iconClass: 'reg reg-attachment',
      name: '4mb.txt',
      url: 'http://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/03e06136-88be-4866-a216-7959afe46137',
    },
    {
      iconClass: 'reg reg-attachment',
      name: '8mb.txt',
      url: 'http://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/11943e38-9fc4-43f6-84c4-12e529eebd28',
    },
    {
      iconClass: 'reg reg-attachment',
      name: '2mb.txt',
      url: 'http://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/092f032d-f54f-49c1-ae42-ebc0d10a2fcb',
    },
    {
      iconClass: 'reg reg-attachment',
      name: '4mb.txt',
      url: 'http://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/8698103b-fad1-4665-85c6-bf88a75ad708',
    },
    {
      iconClass: 'reg reg-attachment',
      name: '8mb.txt',
      url: 'http://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/e950864d-e304-41ca-a60c-0c5019166df8',
    },
    {
      iconClass: 'reg reg-attachment',
      name: '2mb.txt',
      url: 'http://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/005d5bc3-a315-4705-9b06-3788fed86da1',
    },
  ];

  expect(returnInstanceAttachments(instance.data)).toEqual(attachmentsTestData);

});
