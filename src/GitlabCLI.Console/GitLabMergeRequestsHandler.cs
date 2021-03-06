﻿using System.Linq;
using System.Threading.Tasks;
using GitLabCLI.Console.Output;
using GitLabCLI.Core.Gitlab;
using GitLabCLI.Core.Gitlab.Merges;

namespace GitLabCLI.Console
{
    public sealed class GitLabMergeRequestsHandler
    {
        private readonly IGitLabFacade _gitLabFacade;
        private readonly OutputPresenter _presenter;

        public GitLabMergeRequestsHandler(IGitLabFacade gitLabFacade, OutputPresenter presenter)
        {
            _gitLabFacade = gitLabFacade;
            _presenter = presenter;
        }

        public async Task CreateMergeRequest(CreateMergeRequestParameters parameters)
        {
            var mergeRequestResult = await _gitLabFacade.CreateMergeRequest(parameters);
            if (mergeRequestResult.IsFailure)
            {
                _presenter.ShowError("Failed to create merge request", mergeRequestResult.Error);
                return;
            }

            _presenter.ShowSuccess($"Successfully created merge request #{mergeRequestResult.Value}");
        }

        public async Task ListMerges(ListMergesParameters parameters)
        {
            var mergesResult = await _gitLabFacade.ListMergeRequests(parameters);
            if (mergesResult.IsFailure)
            {
                _presenter.ShowError("Failed to retrieve merge requests", mergesResult.Error);
                return;
            }

            var merges = mergesResult.Value;
            if (merges.Count == 0)
            {
                _presenter.ShowSuccess($"No merge requests found in project {parameters.Project}");
                return;
            }

            _presenter.ShowGrid(
                $"Found ({merges.Count}) merge requests in project {parameters.Project}",
                new GridColumn<int>("Merge request Id", 20, merges.Select(s => s.Id)),
                new GridColumn("Title", 100, merges.Select(s => s.Title)),
                new GridColumn("Assignee", 50, merges.Select(s => s.Assignee)));
        }
    }
}
