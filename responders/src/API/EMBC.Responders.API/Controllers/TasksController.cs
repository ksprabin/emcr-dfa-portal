﻿// -------------------------------------------------------------------------
//  Copyright © 2021 Province of British Columbia
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  https://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// -------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EMBC.ESS.Shared.Contracts;
using EMBC.ESS.Shared.Contracts.Events;
using EMBC.Utilities.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EMBC.Responders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly IMessagingClient messagingClient;
        private readonly IMapper mapper;
        private readonly ILogger<TasksController> logger;

        public TasksController(IMessagingClient messagingClient, IMapper mapper, ILogger<TasksController> logger)
        {
            this.messagingClient = messagingClient;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <summary>
        /// Get a single ESS task
        /// </summary>
        /// <param name="taskId">task number</param>
        /// <returns>task or not found</returns>
        [HttpGet("{taskId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<ESSTask>> GetTask(string taskId)
        {
            var reply = await messagingClient.Send(new TasksSearchQuery { TaskId = taskId });
            var task = reply.Items.SingleOrDefault();
            if (task == null) return NotFound(taskId);
            return Ok(mapper.Map<ESSTask>(task));
        }

        [HttpGet("{taskId}/suppliers")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SuppliersListItem>>> GetSuppliersList(string taskId)
        {
            try
            {
                var suppliers = (await messagingClient.Send(new SuppliersListQuery { TaskId = taskId })).Items;
                return Ok(mapper.Map<IEnumerable<SuppliersListItem>>(suppliers));
            }
            catch (NotFoundException)
            {
                return NotFound(taskId);
            }
        }

        [HttpPost("signin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SignIn(string taskId)
        {
            var task = (await messagingClient.Send(new TasksSearchQuery { TaskId = taskId })).Items.SingleOrDefault();
            if (task == null) return NotFound(taskId);
            return Ok();
        }
    }

    public class ESSTask
    {
        public string Id { get; set; } //task number
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CommunityCode { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public IEnumerable<TaskWorkflow> Workflows { get; set; } = Array.Empty<TaskWorkflow>();
    }

    public class TaskWorkflow
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
    }

    public class SuppliersListItem
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }
    }

    public class TaskMapping : Profile
    {
        private static void SetTaskWorkflows(ESSTask task, IncidentTask incidentTask)
        {
            var workflows = new[]
            {
                new TaskWorkflow { Name = "digital-processing",  Enabled = incidentTask.Status == IncidentTaskStatus.Active },
                new TaskWorkflow { Name = "paper-data-entry",  Enabled = true },
                new TaskWorkflow { Name = "remote-extensions",  Enabled = false },
            };
            task.Workflows = workflows;
        }

        public TaskMapping()
        {
            CreateMap<IncidentTask, ESSTask>()
                .ForMember(d => d.Workflows, opts => opts.Ignore())
                .AfterMap((s, d, ctx) =>
                {
                    SetTaskWorkflows(d, s);
                })
                ;
            CreateMap<SupplierDetails, SuppliersListItem>()
                ;
        }
    }
}
